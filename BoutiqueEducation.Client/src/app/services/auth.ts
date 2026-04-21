import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/auth';

  constructor(private http: HttpClient) {}

  login(dto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, dto);
  }

  register(dto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, dto);
  }

  googleLogin(idToken: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/google-login`, { idToken });
  }

  saveToken(token: string) {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout() {
    localStorage.removeItem('token');
  }

  getDecodedToken(): any {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = token.split('.')[1];
      const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(decoded);
    } catch (e) {
      return null;
    }
  }

  getRoles(): string[] {
    const decoded = this.getDecodedToken();
    if (!decoded) return [];
    const roleKey = Object.keys(decoded).find(k => k.endsWith('/role') || k === 'role');
    const roles = roleKey ? decoded[roleKey] : [];
    return Array.isArray(roles) ? roles : [roles];
  }

  isTeacher(): boolean {
    return this.getRoles().includes('Teacher') || this.getRoles().includes('Admin');
  }

  isStudent(): boolean {
    return this.getRoles().includes('Student');
  }

  isAdmin(): boolean {
    return this.getRoles().includes('Admin');
  }

  isApproved(): boolean {
    const decoded = this.getDecodedToken();
    if (!decoded) return false;
    return decoded['isApproved'] === 'true';
  }

  isMember(): boolean {
    return this.getRoles().includes('Uye');
  }

  getFullName(): string {
    const decoded = this.getDecodedToken();
    if (!decoded) return '';
    const nameKey = Object.keys(decoded).find(k =>
      k.endsWith('/name') || k === 'name' || k === 'unique_name'
    );
    return nameKey ? decoded[nameKey] : '';
  }

  getEmail(): string {
    const decoded = this.getDecodedToken();
    if (!decoded) return '';
    const emailKey = Object.keys(decoded).find(k =>
      k.endsWith('/emailaddress') || k === 'email'
    );
    return emailKey ? decoded[emailKey] : '';
  }


  getProfileImageUrl(): string {
    const decoded = this.getDecodedToken();
    if (!decoded) return '';
    return decoded['profileImageUrl'] || '';
  }
}

