import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user';
import Swal from 'sweetalert2';

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
  editForm = { fullName: '', department: '', role: '', newPassword: '', isApproved: false };

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
      role: user.role || '',
      newPassword: '',
      isApproved: user.isApproved || false
    };
    const modal = new bootstrap.Modal(document.getElementById('editUserModal'));
    modal.show();
  }

  approveUser(user: any) {
    Swal.fire({
      title: 'Emin misiniz?',
      text: `${user.fullName} isimli kullanıcıyı onaylamak istiyor musunuz?`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Evet, Onayla',
      cancelButtonText: 'İptal',
      confirmButtonColor: '#6259ca'
    }).then((result) => {
      if (result.isConfirmed) {
        this.userService.updateUser(user.id, { isApproved: true }).subscribe({
          next: () => {
            Swal.fire('Başarılı!', 'Kullanıcı onaylandı.', 'success');
            this.loadUsers();
          },
          error: (err: any) => Swal.fire('Hata!', err.error?.message || 'Onaylanamadı.', 'error')
        });
      }
    });
  }

  submitEdit() {
    if (!this.selectedUser) return;
    this.userService.updateUser(this.selectedUser.id, this.editForm).subscribe({
      next: () => {
        Swal.fire('Başarılı!', 'Kullanıcı bilgileri güncellendi.', 'success');
        this.loadUsers();
        this.blurAndHide('editUserModal');
      },
      error: (err: any) => Swal.fire('Hata!', err.error?.message || 'Güncellenemedi.', 'error')
    });
  }

  private blurAndHide(modalId: string) {
    if (document.activeElement instanceof HTMLElement) document.activeElement.blur();
    const modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    if (modal) modal.hide();
  }
}
