import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, OnDestroy, ChangeDetectorRef } from '@angular/core'; // <-- Hozzáadva: ChangeDetectorRef
import { DailyNoteClient, DailyNoteResponseDto } from '../../shared/models/Nswag generated/NswagGenerated'; 
import { SharedService } from '../../shared/shared.service';
import { Chart, ChartData, ChartOptions, ChartType, registerables } from 'chart.js'; 
import 'chartjs-adapter-moment'; 

interface WeightDataPoint {
  x: number; 
  y: number; 
}
@Component({
  selector: 'app-weight-history',
  standalone: false,
  templateUrl: './weight-history.component.html',
  styleUrls: ['./weight-history.component.css'] 
})
export class WeightHistoryComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('weightChart') private chartRef?: ElementRef<HTMLCanvasElement>;
  
  public loading = true;
  public weightHistory: DailyNoteResponseDto[] = []; 
  private weightChart?: Chart;
  public errorMessage: string | null = null;

  public lineChartData: ChartData<'line', WeightDataPoint[], number> = {
    datasets: []
  };

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
          text: 'Date' 
        }
      },
      y: {
        title: {
          display: true,
          text: 'Weight (kg)' 
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
            return `Weight: ${context.parsed.y} kg`; 
          }
        }
      }
    }
  };

  public lineChartType: ChartType = 'line';

  constructor(
    private dailyNoteClient: DailyNoteClient,
    private sharedService: SharedService,
    private cdr: ChangeDetectorRef 
  ) { 

    Chart.register(...registerables);
  }
  
  ngOnInit(): void {
  }
  
  ngAfterViewInit(): void {
  }

  ngOnDestroy(): void {
    if (this.weightChart) {
      this.weightChart.destroy();
    }
  }

  public async loadWeightHistory(): Promise<void> {
    this.loading = true;
    this.errorMessage = null;
    if (this.weightChart) {
        this.weightChart.destroy();
        this.weightChart = undefined;
    }
    

    this.cdr.detectChanges(); 

    
    try {
        const notes = await this.dailyNoteClient.getWeightHistory().toPromise();
        
        if (!notes) {
            this.errorMessage = 'error'; 
            this.loading = false;
            this.cdr.detectChanges(); 
            return;
        }
        
        this.weightHistory = notes.filter(n => (n.dailyWeight ?? 0) > 0); 
        
        if (this.weightHistory.length >= 2) {
          await this.prepareChartData(); 
        } else {
           this.loading = false;
           this.cdr.detectChanges();
        }

    } catch (error) {
        console.error('[WEIGHT CHART] Error fetching weight data:', error);
        this.errorMessage = 'error'; 
        this.sharedService.showNotification(false, 'error', this.errorMessage);
        this.loading = false;
        this.cdr.detectChanges(); 
    }
  }
  private async prepareChartData(): Promise<void> {
    const dataPoints: WeightDataPoint[] = [];
    
    
    this.weightHistory.forEach(note => {
    
      const timestamp = new Date(note.createdAt!).getTime(); 
      dataPoints.push({ x: timestamp, y: note.dailyWeight! }); 
    });


   
    this.lineChartData = {
      datasets: [
        {
          data: dataPoints, 
          label: 'Weight',
          borderColor: '#0d6efd', 
          backgroundColor: 'rgba(13, 110, 253, 0.2)',
          pointBackgroundColor: '#0d6efd',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: '#0d6efd',
          fill: 'origin',
          tension: 0.4 
        }
      ]
    };

    this.updateChartOptions(dataPoints);
    this.loading = false;
    this.cdr.detectChanges(); 

    await new Promise(resolve => setTimeout(resolve, 50)); 

    this.createChart();
  }
  
  private updateChartOptions(dataPoints: WeightDataPoint[]): void {
      if (!this.lineChartOptions?.scales?.['y']) return;
      const weights = dataPoints.map(dp => dp.y);
      if (weights.length > 0) {
        const minWeight = Math.min(...weights) - 5;
        this.lineChartOptions.scales['y'].min = minWeight > 0 ? minWeight : 50; 
      }
  }

  private createChart(): void {
    if (this.weightChart) {
      this.weightChart.destroy();
    }
    
    
    const ctx = this.chartRef?.nativeElement?.getContext('2d');
    if (ctx) {
  
      this.weightChart = new Chart<'line', WeightDataPoint[], number>(ctx, {
        type: this.lineChartType as 'line',
        data: this.lineChartData,
        options: this.lineChartOptions
      });
    } else {
        console.error('null');
    }
  }
}
