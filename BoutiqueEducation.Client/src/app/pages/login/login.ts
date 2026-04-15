import { Component, OnInit, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { environment } from '../../../environments/environment';

declare const google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {
  dto = { email: '', password: '' };
  isLoading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private ngZone: NgZone
  ) {}

  ngOnInit() {
    this.loadGoogleScript().then(() => this.initGoogleSignIn());
  }

  loadGoogleScript(): Promise<void> {
    return new Promise((resolve) => {
      if (document.getElementById('google-gsi-script')) {
        resolve();
        return;
      }
      const script = document.createElement('script');
      script.id = 'google-gsi-script';
      script.src = 'https://accounts.google.com/gsi/client';
      script.async = true;
      script.defer = true;
      script.onload = () => resolve();
      document.head.appendChild(script);
    });
  }

  initGoogleSignIn() {
    if (typeof google === 'undefined') return;

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => {
        // NgZone içinde çalıştırarak Angular change detection'ı tetikle
        this.ngZone.run(() => {
          this.handleGoogleToken(response.credential);
        });
      },
      auto_select: false,
      cancel_on_tap_outside: true
    });

    // Butonu render et
    google.accounts.id.renderButton(
      document.getElementById('google-signin-btn'),
      {
        type: 'standard',
        shape: 'rectangular',
        theme: 'outline',
        text: 'signin_with',
        size: 'large',
        logo_alignment: 'left',
        width: 360   // px cinsinden — GIS yüzde kabul etmiyor
      }
    );
  }

  onSubmit() {
    if (!this.dto.email || !this.dto.password) return;
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.dto).subscribe({
      next: (res: any) => {
        const token = res?.data?.accessToken || res?.accessToken || res?.token;
        if (token) {
          this.authService.saveToken(token);
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage = 'Token alınamadı.';
          this.isLoading = false;
        }
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'E-posta veya şifre hatalı.';
      }
    });
  }

  handleGoogleToken(idToken: string) {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.googleLogin(idToken).subscribe({
      next: (res: any) => {
        const token = res?.data?.accessToken || res?.accessToken;
        if (token) {
          this.authService.saveToken(token);
          this.router.navigate(['/dashboard']);
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Google ile giriş başarısız.';
      }
    });
  }
}
