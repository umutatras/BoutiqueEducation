import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService, DashboardStats } from '../../services/dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  stats: DashboardStats | null = null;
  isLoading: boolean = false;

  constructor(private ds: DashboardService) {}

  ngOnInit() {
    this.isLoading = true;
    this.ds.getStats().subscribe({
      next: (res: any) => {
        // Backend returns Result<T>, so actual data is in res.data
        this.stats = res.data || res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Stats fetching failed:', err);
        this.isLoading = false;
      }
    });
  }
}
