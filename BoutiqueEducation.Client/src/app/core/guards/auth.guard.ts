import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth';
import Swal from 'sweetalert2';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.getToken()) {
    router.navigate(['/login']);
    return false;
  }

  if (!authService.isApproved() && !authService.isAdmin()) {
    Swal.fire({
      icon: 'warning',
      title: 'Hoppala!',
      text: 'Üyeliğiniz henüz onaylanmamış. Onaylandıktan sonra tüm sayfalar aktif olacaktır.',
      confirmButtonText: 'Tamam',
      confirmButtonColor: '#6259ca'
    }).then(() => {
      authService.logout();
      router.navigate(['/login']);
    });
    return false;
  }

  return true;
};
