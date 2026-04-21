import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';
import { finalize } from 'rxjs';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  dto = { fullName: '', email: '', password: '', confirmPassword: '' };
  isLoading = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    this.errorMessage = '';
    if (!this.dto.fullName || !this.dto.email || !this.dto.password) {
      this.errorMessage = 'Tüm alanları doldurun.';
      return;
    }
    if (this.dto.password !== this.dto.confirmPassword) {
      this.errorMessage = 'Şifreler eşleşmiyor.';
      return;
    }
    if (this.dto.password.length < 6) {
      this.errorMessage = 'Şifre en az 6 karakter olmalıdır.';
      return;
    }

    this.isLoading = true;
    this.authService.register({
      fullName: this.dto.fullName,
      email: this.dto.email,
      password: this.dto.password
    })
    .pipe(finalize(() => this.isLoading = false))
    .subscribe({
      next: () => {
        Swal.fire({
          icon: 'success',
          title: 'Kayıt Başarılı!',
          text: 'Hesabınız oluşturuldu. Yönetici onayından sonra sisteme giriş yapabilirsiniz.',
          confirmButtonText: 'Giriş Sayfasına Dön',
          confirmButtonColor: '#6259ca'
        }).then(() => {
          this.router.navigate(['/login']);
        });
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Kayıt sırasında bir hata oluştu.';
      }
    });
  }
}
