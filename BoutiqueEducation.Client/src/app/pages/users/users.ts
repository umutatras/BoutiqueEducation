import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user';

declare var bootstrap: any;

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.html',
})
export class Users implements OnInit {
  users: any[] = [];
  isLoading = false;

  selectedUser: any = null;
  editForm = { fullName: '', department: '', role: '', newPassword: '' };

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (res: any) => {
        this.users = res?.data || res || [];
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  openEditModal(user: any) {
    this.selectedUser = user;
    this.editForm = {
      fullName: user.fullName || '',
      department: user.department || '',
      role: '', // Rolü şimdilik boş, backend mevcut rolü değiştirmiyor eğer boşsa
      newPassword: ''
    };
    const modal = new bootstrap.Modal(document.getElementById('editUserModal'));
    modal.show();
  }

  submitEdit() {
    if (!this.selectedUser) return;
    this.userService.updateUser(this.selectedUser.id, this.editForm).subscribe({
      next: () => {
        this.loadUsers();
        this.blurAndHide('editUserModal');
      },
      error: (err: any) => alert('Güncellenemedi: ' + (err.error?.message || 'Hata'))
    });
  }

  private blurAndHide(modalId: string) {
    if (document.activeElement instanceof HTMLElement) document.activeElement.blur();
    const modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    if (modal) modal.hide();
  }
}
