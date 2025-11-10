import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { BarberInfo } from '../../models/models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  barberInfo?: BarberInfo;
  loading = true;
  error?: string;

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadBarberInfo();
  }

  loadBarberInfo(): void {
    this.apiService.getBarberInfo().subscribe({
      next: (data) => {
        this.barberInfo = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading barber info:', err);
        this.error = 'Failed to load barber information';
        this.loading = false;
      }
    });
  }
}
