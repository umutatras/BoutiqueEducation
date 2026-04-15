import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Contact {
  id: string;
  fullName: string;
  email: string;
  department?: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = environment.apiUrl + '/users';

  constructor(private http: HttpClient) {}

  getContacts(): Observable<any> {
    return this.http.get(`${this.apiUrl}/contacts`);
  }

  getAllUsers(): Observable<any> {
    return this.http.get(`${this.apiUrl}/all`);
  }

  updateUser(userId: string, dto: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${userId}`, dto);
  }

  changePassword(dto: { currentPassword: string; newPassword: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/change-password`, dto);
  }

  getLastMessage(): Observable<any> {
    return this.http.get(`${this.apiUrl}/last-message`);
  }
}
