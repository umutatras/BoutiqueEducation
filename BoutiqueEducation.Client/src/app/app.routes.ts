import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { MainLayout } from './layout/main-layout/main-layout';
import { Dashboard } from './pages/dashboard/dashboard';
import { Questions } from './pages/questions/questions';
import { Tasks } from './pages/tasks/tasks';
import { Chat } from './pages/chat/chat';
import { Users } from './pages/users/users';

import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'questions', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard, canActivate: [adminGuard] },
      { path: 'users', component: Users, canActivate: [adminGuard] },
      { path: 'questions', component: Questions },
      { path: 'tasks', component: Tasks },
      { path: 'chat', component: Chat }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
