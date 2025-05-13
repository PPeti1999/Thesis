import { Component, OnInit } from '@angular/core';
import { AccountClient } from '../../shared/models/Nswag generated/NswagGenerated';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-daily-note',
  standalone: false,
  templateUrl: './daily-note.component.html',
  styleUrl: './daily-note.component.css'
})
export class DailyNoteComponent implements OnInit{
  constructor(
    private route: ActivatedRoute,
    private userClient: AccountClient,
    private router: Router
  ) {}


  ngOnInit(): void {
    this.userClient.getProfile().subscribe(profile => {
      
      const missingProfile = !profile.age || !profile.height || !profile.weight;
      if (missingProfile) {
        this.router.navigate(['/create-profile']);
      } else {
        // ide jönne a dailynote betöltése
      }
    });
  }

}
