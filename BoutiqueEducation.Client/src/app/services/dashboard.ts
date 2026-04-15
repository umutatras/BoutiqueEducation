import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DashboardStats {
  totalQuestions: number;
  pendingQuestions: number;
  answeredQuestions: number;
  totalTasks: number;
  pendingTasks: number;
  submittedTasks: number;
  approvedTasks: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = environment.apiUrl + '/dashboard/stats';

  constructor(private http: HttpClient) {}

  getStats(): Observable<DashboardStats> {
     // Expected to return { data: DashboardStats, ... } based on BaseController
    return this.http.get<DashboardStats>(this.apiUrl);
  }
}
