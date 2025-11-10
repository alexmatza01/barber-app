import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { PortfolioItem } from '../../models/models';

@Component({
  selector: 'app-portfolio',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit {
  portfolioItems: PortfolioItem[] = [];
  loading = true;
  error?: string;

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadPortfolio();
  }

  loadPortfolio(): void {
    this.apiService.getPortfolio().subscribe({
      next: (data) => {
        this.portfolioItems = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading portfolio:', err);
        this.error = 'Failed to load portfolio';
        this.loading = false;
      }
    });
  }
}
