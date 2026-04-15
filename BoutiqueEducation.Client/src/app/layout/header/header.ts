import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';
import { UserService } from '../../services/user';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, DatePipe],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header implements OnInit {
  userName: string = '';
  userRole: string = '';
  userEmail: string = '';
  isDarkMode: boolean = false;

  lastMessage: any = null;
  lastMessageSenderName: string = '';

  // Şifre değiştirme
  pwForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
  pwError = '';
  pwSuccess = '';

  constructor(
    public authService: AuthService,
    private router: Router,
    private userService: UserService
  ) {}

  ngOnInit() {
    this.userName = this.authService.getFullName() || this.authService.getEmail() || 'Kullanıcı';
    this.userEmail = this.authService.getEmail();
    const roles = this.authService.getRoles();
    this.userRole = roles.length ? roles[0] : '';

    // Dark mode: localStorage'dan oku
    this.isDarkMode = localStorage.getItem('darkMode') === 'true';
    this.applyDarkMode();

    // Son mesajı yükle
    this.loadLastMessage();
  }

  loadLastMessage() {
    this.userService.getLastMessage().subscribe({
      next: (res: any) => {
        this.lastMessage = res?.data || null;
      },
      error: () => {}
    });
  }

  goToChat() {
    this.router.navigate(['/chat']);
  }

  toggleDarkMode(event: Event) {
    event.stopPropagation(); // prevent dropdown close if needed
    this.isDarkMode = !this.isDarkMode;
    localStorage.setItem('darkMode', String(this.isDarkMode));
    this.applyDarkMode();
  }

  applyDarkMode() {
    const body = document.body;
    if (this.isDarkMode) {
      body.classList.remove('light-mode');
      body.classList.add('dark-mode');
    } else {
      body.classList.remove('dark-mode');
      body.classList.add('light-mode');
    }
  }

  submitChangePassword() {
    this.pwError = '';
    this.pwSuccess = '';
    if (this.pwForm.newPassword !== this.pwForm.confirmPassword) {
      this.pwError = 'Yeni şifreler eşleşmiyor.';
      return;
    }
    this.userService.changePassword({
      currentPassword: this.pwForm.currentPassword,
      newPassword: this.pwForm.newPassword
    }).subscribe({
      next: () => {
        this.pwSuccess = 'Şifreniz başarıyla değiştirildi.';
        this.pwForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
      },
      error: (err: any) => {
        this.pwError = err.error?.message || 'Şifre değiştirilemedi.';
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
