import { Component, OnInit } from '@angular/core';
import { ActivityCatalogClient, ActivityCatalogCreateDto, ActivityCatalogResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';

@Component({
  selector: 'app-create-activity-catalog',
  standalone: false,
  templateUrl: './create-activity-catalog.component.html',
  styleUrl: './create-activity-catalog.component.css'
})
export class CreateActivityCatalogComponent {
  activity: ActivityCatalogCreateDto = new ActivityCatalogCreateDto();
  editing = false;
  activityId: string | null = null;
  showAlert = false;
  //text:any;

  constructor(
    private activityClient: ActivityCatalogClient,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.route.paramMap.pipe(take(1)).subscribe(params => {
      this.activityId = params.get('id');
      if (this.activityId) {
        this.editing = true;
        console.log('Szerkesztés módban vagyunk, ID:', this.activityId);
        this.activityClient.getById(this.activityId).subscribe({
          next: (data: ActivityCatalogResponseDto) => {
            this.activity = new ActivityCatalogCreateDto();
            Object.assign(this.activity, {
              name: data.name,
              minute: data.minute,
              calories: data.calories
            });
          },
          error: err => {
            console.error('Nem sikerült lekérni az aktivitást:', err);
            this.router.navigate(['/activitycatalog']);
          }
        });
      } else {
        console.log('Új aktivitás létrehozása módban vagyunk.');
      }
    });
  }

  beforeSendClean(dto: any): void {
    for (const key in dto) {
      if (dto[key] === undefined || dto[key] === null || dto[key] === '') {
        delete dto[key];
      }
    }
  }

  createOrUpdate(): void {
    const cleanDto = new ActivityCatalogCreateDto();
    Object.assign(cleanDto, this.activity);
    //this.beforeSendClean(cleanDto);

    if (this.editing && this.activityId) {
      console.log('Frissítés DTO:', cleanDto);
      this.activityClient.update(this.activityId, cleanDto).subscribe({
        next: () => {
          console.log('Sikeres frissítés');
          this.router.navigate(['/activitycatalog']);
        },
        error: err => {
          console.error('Frissítés sikertelen:', err);
        }
      });
    } else {
      console.log('Létrehozás DTO:', cleanDto);
      console.log('Beküldendő JSON:', JSON.stringify(cleanDto));

      this.activityClient.create(cleanDto).subscribe({
        next: () => {
          console.log('Sikeres létrehozás');
          this.router.navigate(['/activitycatalog']);
        },
        error: err => {
          if (err.error instanceof Blob) {
            err.error.text().then((text: string) => {
              try {
                const parsed = JSON.parse(text);
                console.error('ModelState hiba:', parsed);
              } catch {
                console.error('Nyers válasz:', text);
              }
            });
          } else {
            console.error('Létrehozás sikertelen:', err);
          }
        }
      });
    }
  }
}