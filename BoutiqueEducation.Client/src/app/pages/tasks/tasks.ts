import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task';
import { AuthService } from '../../services/auth';
import { UserService } from '../../services/user';
import { FileService } from '../../services/file';
import { environment } from '../../../environments/environment';
import { Calendar } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';

declare var bootstrap: any;

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks.html',
  styleUrl: './tasks.css'
})
export class Tasks implements OnInit, AfterViewInit {
  @ViewChild('calendarEl', { static: false }) calendarRef!: ElementRef;

  tasks: any[] = [];
  isLoading = false;
  isLoadingStudents = false;
  calendar!: Calendar;

  selectedTask: any = null;
  newTask = { title: '', description: '', studentId: '', dueDate: '' };
  submitNotes: string = '';
  selectedFile: File | null = null;
  public apiUrl = environment.apiUrl.replace('/api', '');

  // Öğrenci listesi (öğretmen için)
  students: any[] = [];

  constructor(
    private taskService: TaskService,
    public authService: AuthService,
    private userService: UserService,
    private fileService: FileService
  ) {}

  ngOnInit() {
    this.loadTasks();
    if (this.authService.isTeacher()) {
      this.loadStudents();
    }
  }

  ngAfterViewInit() {
    setTimeout(() => this.initCalendar(), 100);
  }

  initCalendar() {
    if (!this.calendarRef?.nativeElement) return;

    this.calendar = new Calendar(this.calendarRef.nativeElement, {
      plugins: [dayGridPlugin, interactionPlugin],
      initialView: 'dayGridMonth',
      locale: 'tr',
      headerToolbar: {
        left: 'prev,next today',
        center: 'title',
        right: 'dayGridMonth,dayGridWeek'
      },
      buttonText: {
        today: 'Bugün',
        month: 'Ay',
        week: 'Hafta'
      },
      height: 'auto',
      dateClick: (info: any) => {
        if (this.authService.isTeacher()) {
          this.newTask = { title: '', description: '', studentId: '', dueDate: info.dateStr };
          const modal = new bootstrap.Modal(document.getElementById('assignTaskModal'));
          modal.show();
        }
      },
      eventClick: (info: any) => {
        const taskId = info.event.id;
        const task = this.tasks.find((t: any) => t.id === taskId);
        if (task) {
          this.openViewModal(task);
        }
      },
      events: []
    });
    this.calendar.render();
  }

  loadStudents() {
    this.isLoadingStudents = true;
    this.userService.getContacts().subscribe({
      next: (res: any) => {
        this.students = res?.data || res || [];
        this.isLoadingStudents = false;
      },
      error: () => this.isLoadingStudents = false
    });
  }

  loadTasks() {
    this.isLoading = true;
    this.taskService.getStudentTasks().subscribe({
      next: (res: any) => {
        if (Array.isArray(res)) {
          this.tasks = res;
        } else if (Array.isArray(res?.data)) {
          this.tasks = res.data;
        } else {
          this.tasks = [];
        }
        this.isLoading = false;
        this.updateCalendarEvents();
      },
      error: () => this.isLoading = false
    });
  }

  updateCalendarEvents() {
    if (!this.calendar) return;

    // Mevcut eventleri temizle
    this.calendar.removeAllEvents();

    this.tasks.forEach((t: any) => {
      const eventDate = t.dueDate || t.createdDate;
      if (!eventDate) return;

      let color = '#3788d8'; // Pending - mavi
      if (t.status === 'Submitted') color = '#f59f00'; // Sarı
      if (t.status === 'Approved') color = '#2fb344'; // Yeşil
      if (t.status === 'Rejected') color = '#d63939'; // Kırmızı

      const displayName = this.authService.isTeacher()
        ? `📋 ${t.studentName}: ${t.title}`
        : `📋 ${t.title}`;

      this.calendar.addEvent({
        id: t.id,
        title: displayName,
        start: eventDate,
        backgroundColor: color,
        borderColor: color,
        allDay: true
      });
    });
  }

  openViewModal(t: any) {
    this.selectedTask = t;
    this.submitNotes = '';
    this.selectedFile = null;
    const modal = new bootstrap.Modal(document.getElementById('viewTaskModal'));
    modal.show();
  }

  openAssignModal() {
    this.newTask = { title: '', description: '', studentId: '', dueDate: '' };
    const modal = new bootstrap.Modal(document.getElementById('assignTaskModal'));
    modal.show();
  }

  assignTask() {
    if (!this.newTask.title || !this.newTask.studentId) return;
    const payload: any = {
      title: this.newTask.title,
      description: this.newTask.description,
      studentId: this.newTask.studentId
    };
    if (this.newTask.dueDate) {
      payload.dueDate = this.newTask.dueDate;
    }
    this.taskService.assignTask(payload).subscribe({
      next: () => {
        this.loadTasks();
        this.blurAndHide('assignTaskModal');
      },
      error: (err: any) => alert('Ödev atanamadı: ' + (err.error?.message || ''))
    });
  }

  onFileSelected(event: any) {
    if (event.target.files.length > 0) {
      this.selectedFile = event.target.files[0];
    }
  }

  submitTask() {
    if (!this.selectedTask) return;

    if (this.selectedFile) {
      this.fileService.upload(this.selectedFile).subscribe({
        next: (res: any) => {
          this.finalizeSubmitTask(res.url); // Ext is handled backend, FileUrl is returned
        },
        error: () => alert('Dosya yüklenemedi!')
      });
    } else {
      this.finalizeSubmitTask(null);
    }
  }

  private finalizeSubmitTask(fileUrl: string | null) {
    this.taskService.submitTask(this.selectedTask.id, { 
      fileUrl: fileUrl,
      additionalNotes: this.submitNotes 
    }).subscribe({
      next: () => {
        this.loadTasks();
        this.blurAndHide('viewTaskModal');
      },
      error: () => alert('Ödev teslim edilemedi.')
    });
  }

  approveTask(t: any) {
    if (!confirm('Bu ödevi onaylamak istiyor musunuz?')) return;
    this.taskService.approveTask(t.id).subscribe({
      next: () => this.loadTasks(),
      error: () => alert('Onaylanırken hata oluştu.')
    });
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'Bekliyor';
      case 'Submitted': return 'Teslim Edildi';
      case 'Approved': return 'Onaylandı';
      case 'Rejected': return 'Reddedildi';
      default: return status;
    }
  }

  private blurAndHide(modalId: string) {
    if (document.activeElement instanceof HTMLElement) document.activeElement.blur();
    const modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    if (modal) modal.hide();
  }
}
