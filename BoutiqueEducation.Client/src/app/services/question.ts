import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalCount: number;
}

export const CATEGORIES = ['Genel', 'Matematik', 'Fizik', 'Biyoloji', 'Kimya', 'Fen', 'Tarih', 'İngilizce', 'Türkçe', 'Coğrafya'];
export const DEPARTMENTS = ['Matematik', 'Fizik', 'Biyoloji', 'Kimya', 'Fen', 'Tarih', 'İngilizce', 'Türkçe', 'Coğrafya'];

@Injectable({ providedIn: 'root' })
export class QuestionService {
  private apiUrl = environment.apiUrl + '/questions';

  constructor(private http: HttpClient) {}

  getAll(pageNumber = 1, pageSize = 10): Observable<any> {
    const params = new HttpParams()
      .set('PageNumber', pageNumber)
      .set('PageSize', pageSize);
    return this.http.get<any>(this.apiUrl, { params });
  }

  create(dto: any): Observable<any> {
    return this.http.post(this.apiUrl, dto);
  }

  answer(id: string, dto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/answer`, dto);
  }
}
