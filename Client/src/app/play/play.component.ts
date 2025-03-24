import { Component } from '@angular/core';
import { PlayService } from './play.service';

@Component({
  selector: 'app-play',
  standalone: false,
  templateUrl: './play.component.html',
  styleUrl: './play.component.css'
})
export class PlayComponent {
  message: string | undefined;

  constructor(private playService: PlayService) {}

  ngOnInit(): void {
    this.playService.getPlayers().subscribe({
      next: (respose: any) => this.message = respose.value.message,
      error: error => console.log(error)
    })
  }
}
