import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, OnDestroy, ChangeDetectorRef } from '@angular/core'; // <-- Hozzáadva: ChangeDetectorRef
import { DailyNoteClient, DailyNoteResponseDto } from '../../shared/models/Nswag generated/NswagGenerated'; 
import { SharedService } from '../../shared/shared.service';
import { Chart, ChartData, ChartOptions, ChartType, registerables } from 'chart.js'; 
import 'chartjs-adapter-moment'; 

// Data point format for Chart.js time scale
interface WeightDataPoint {
  x: number; // Date as Unix timestamp (milliseconds)
  y: number; // Weight (kg)
}

@Component({
  selector: 'app-weight-history',
  standalone: false,
  templateUrl: './weight-history.component.html',
  // The Angular style handling
  styleUrls: ['./weight-history.component.css'] 
})
export class WeightHistoryComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('weightChart') private chartRef?: ElementRef<HTMLCanvasElement>;
  
  public loading = true;
  public weightHistory: DailyNoteResponseDto[] = []; 
  private weightChart?: Chart;
  public errorMessage: string | null = null;

  // ChartData uses generic parameters to satisfy strict TypeScript checking
  public lineChartData: ChartData<'line', WeightDataPoint[], number> = {
    datasets: []
  };

  // ChartOptions uses generic parameters
  public lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false, 
    scales: {
      x: {
        type: 'time',
        time: {
          unit: 'day',
          displayFormats: {
            day: 'MMM D'
          }
        },
        title: {
          display: true,
          text: 'Date' // Date
        }
      },
      y: {
        title: {
          display: true,
          text: 'Weight (kg)' // Weight (kg)
        },
        beginAtZero: false
      }
    },
    plugins: {
      legend: {
        display: false
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            return `Weight: ${context.parsed.y} kg`; // Weight: Y kg
          }
        }
      }
    }
  };

  public lineChartType: ChartType = 'line';

  constructor(
    private dailyNoteClient: DailyNoteClient,
    private sharedService: SharedService,
    private cdr: ChangeDetectorRef // <-- INJEKTÁLVA: ChangeDetectorRef a kézi frissítéshez
  ) { 
    // Register all Chart.js modules
    Chart.register(...registerables);
  }
  
  // A komponens ne indítsa el automatikusan a betöltést, mert egy modálban van,
  // és meg kell várnia a szülő (DailyNoteComponent) hívását.
  ngOnInit(): void {
    console.log('[WEIGHT CHART] Component initialized. No auto-load in modal context.');
    // Eltávolítva: this.loadWeightHistory();
  }
  
  ngAfterViewInit(): void {
    // Ensures cleanup happens if the component is used dynamically
  }

  ngOnDestroy(): void {
    // Clean up chart instance to prevent memory leaks
    if (this.weightChart) {
      this.weightChart.destroy();
    }
  }

  // Public method called by the parent (DailyNoteComponent) to refresh data
  public async loadWeightHistory(): Promise<void> {
    this.loading = true;
    this.errorMessage = null;
    // Ensure cleanup of previous chart before loading new data
    if (this.weightChart) {
        this.weightChart.destroy();
        this.weightChart = undefined;
    }
    
    // Kényszerítjük az Angular frissítését, hogy a betöltő animáció megjelenjen
    this.cdr.detectChanges(); 

    console.log('[WEIGHT CHART] API call started for weight history.');
    
    try {
        const notes = await this.dailyNoteClient.getWeightHistory().toPromise();
        
        if (!notes) {
            this.errorMessage = 'Súly adatok lekérése sikertelen.'; // Failed to fetch weight data.
            this.loading = false;
            this.cdr.detectChanges(); // Frissítjük a nézetet a hibaüzenettel
            return;
        }

        console.log('[WEIGHT CHART] API returned data:', notes);
        
        // Filter notes with valid weight (greater than 0)
        this.weightHistory = notes.filter(n => (n.dailyWeight ?? 0) > 0); 
        
        if (this.weightHistory.length >= 2) {
          await this.prepareChartData(); // Use await to ensure data preparation completes before chart creation
        } else {
           console.log('[WEIGHT CHART] Not enough data points to display chart (min. 2 required).');
           this.loading = false;
           this.cdr.detectChanges(); // Frissítjük a nézetet a "Nincs adat" üzenettel
        }

    } catch (error) {
        console.error('[WEIGHT CHART] Error fetching weight data:', error);
        this.errorMessage = 'Súly adatok lekérése sikertelen a szerverről.'; // Failed to fetch weight data from server.
        this.sharedService.showNotification(false, 'Hiba', this.errorMessage);
        this.loading = false;
        this.cdr.detectChanges(); // Frissítjük a nézetet a hibaüzenettel
    }
  }

  // Async data preparation to allow for awaiting DOM updates
  private async prepareChartData(): Promise<void> {
    const dataPoints: WeightDataPoint[] = [];
    
    // Process data and convert Date object to timestamp
    this.weightHistory.forEach(note => {
      // NSwag returns date strings, which Angular/JS converts. We convert that Date object to a timestamp (number).
      // Note: `note.createdAt` is expected to be a Date object here.
      const timestamp = new Date(note.createdAt!).getTime(); 
      dataPoints.push({ x: timestamp, y: note.dailyWeight! }); 
    });

    console.log('[WEIGHT CHART] Prepared Chart Data Points:', dataPoints);

    // Update ChartData object
    this.lineChartData = {
      datasets: [
        {
          data: dataPoints, 
          label: 'Testsúly', // Weight
          borderColor: '#0d6efd', // Bootstrap primary blue
          backgroundColor: 'rgba(13, 110, 253, 0.2)',
          pointBackgroundColor: '#0d6efd',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: '#0d6efd',
          fill: 'origin', // Fill area to origin
          tension: 0.4 
        }
      ]
    };

    this.updateChartOptions(dataPoints);
    this.loading = false;
    
    // 1. KRITIKUS LÉPÉS: Kényszerítjük az Angular frissítését, hogy az `*ngIf`
    // megjelenítse a `<canvas>` elemet a DOM-ban.
    this.cdr.detectChanges(); 
    
    // 2. Megtartjuk a kis késleltetést, mint másodlagos védelmet
    // a Bootstrap modál DOM reflow eseményeivel szemben.
    await new Promise(resolve => setTimeout(resolve, 50)); 

    this.createChart();
  }
  
  private updateChartOptions(dataPoints: WeightDataPoint[]): void {
      if (!this.lineChartOptions?.scales?.['y']) return;
      
      // Dynamically set Y-axis minimum (min weight - 5kg)
      const weights = dataPoints.map(dp => dp.y);
      if (weights.length > 0) {
        const minWeight = Math.min(...weights) - 5;
        // Ensure min is not too low (e.g., 50kg)
        this.lineChartOptions.scales['y'].min = minWeight > 0 ? minWeight : 50; 
      }
  }

  private createChart(): void {
    if (this.weightChart) {
      this.weightChart.destroy();
    }
    
    // Most már a this.cdr.detectChanges() és a setTimeout miatt a chartRef?.nativeElement már nem lehet null
    const ctx = this.chartRef?.nativeElement?.getContext('2d');
    if (ctx) {
      // Explicitly pass generic types to satisfy TS compiler
      this.weightChart = new Chart<'line', WeightDataPoint[], number>(ctx, {
        type: this.lineChartType as 'line',
        data: this.lineChartData,
        options: this.lineChartOptions
      });
      console.log('[WEIGHT CHART] Chart successfully initialized.');
    } else {
        console.error('[WEIGHT CHART] Canvas context not found (chartRef is null).');
    }
  }
}
