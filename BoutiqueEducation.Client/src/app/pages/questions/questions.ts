import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { QuestionService, CATEGORIES } from '../../services/question';
import { AuthService } from '../../services/auth';
import { FileService } from '../../services/file';
import { environment } from '../../../environments/environment';

declare var bootstrap: any;

@Component({
  selector: 'app-questions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './questions.html',
  styleUrl: './questions.css'
})
export class Questions implements OnInit {
  questions: any[] = [];
  pageNumber = 1;
  pageSize = 10;
  totalPageCount = 0;
  isLoading = false;
  errorMessage = '';

  selectedQuestion: any = null;
  newQuestion = { content: '', category: 'Genel', imageUrl: '' };
  answerContent = '';
  selectedQuestionFile: File | null = null;
  selectedAnswerFile: File | null = null;
  public apiUrl = environment.apiUrl.replace('/api', '');

  readonly categories = CATEGORIES;

  constructor(
    public questionService: QuestionService, 
    public authService: AuthService,
    private fileService: FileService
  ) {}

  ngOnInit() { this.loadQuestions(); }

  loadQuestions() {
    this.isLoading = true;
    this.errorMessage = '';
    this.questionService.getAll(this.pageNumber, this.pageSize).subscribe({
      next: (res: any) => {
        console.log('Questions API response:', res);
        // HandleResult -> Ok(PagedResponse) → res = { data: [...], totalPageCount, ... }
        // Tüm olasılıkları destekle:
        if (Array.isArray(res)) {
          this.questions = res;
        } else if (Array.isArray(res?.data)) {
          // res = PagedResponse doğrudan: { data: [...], totalPageCount: N }
          this.questions = res.data;
          this.totalPageCount = res.totalPageCount ?? 1;
        } else if (Array.isArray(res?.items)) {
          // Eski field ismi ile gelme ihtimali
          this.questions = res.items;
          this.totalPageCount = res.totalPages ?? 1;
        } else {
          this.questions = [];
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Questions API error:', err);
        this.errorMessage = err.error?.message || 'Sorular yüklenemedi.';
        this.isLoading = false;
      }
    });
  }

  openViewModal(q: any) {
    this.selectedQuestion = q;
    this.answerContent = '';
    const modal = new bootstrap.Modal(document.getElementById('viewQuestionModal'));
    modal.show();
  }

  openAskModal() {
    this.newQuestion = { content: '', category: 'Genel', imageUrl: '' };
    this.selectedQuestionFile = null;
    const modal = new bootstrap.Modal(document.getElementById('askQuestionModal'));
    modal.show();
  }

  onQuestionFileSelected(event: any) {
    if (event.target.files.length > 0) {
      this.selectedQuestionFile = event.target.files[0];
    }
  }

  submitQuestion() {
    if (!this.newQuestion.content.trim()) return;

    if (this.selectedQuestionFile) {
      this.fileService.upload(this.selectedQuestionFile).subscribe({
        next: (res: any) => {
          this.newQuestion.imageUrl = res.url;
          this.finalizeSubmitQuestion();
        },
        error: () => alert('Dosya yüklenemedi.')
      });
    } else {
      this.finalizeSubmitQuestion();
    }
  }

  private finalizeSubmitQuestion() {
    this.questionService.create(this.newQuestion).subscribe({
      next: () => {
        this.loadQuestions();
        this.blurAndHide('askQuestionModal');
      },
      error: (err: any) => alert('Soru gönderilemedi: ' + (err.error?.message || err.message))
    });
  }

  onAnswerFileSelected(event: any) {
    if (event.target.files.length > 0) {
      this.selectedAnswerFile = event.target.files[0];
    }
  }

  submitAnswer() {
    if (!this.selectedQuestion || !this.answerContent.trim()) return;

    if (this.selectedAnswerFile) {
      this.fileService.upload(this.selectedAnswerFile).subscribe({
        next: (res: any) => {
          this.finalizeSubmitAnswer(res.url);
        },
        error: () => alert('Dosya yüklenemedi.')
      });
    } else {
      this.finalizeSubmitAnswer('');
    }
  }

  private finalizeSubmitAnswer(imageUrl: string) {
    this.questionService.answer(this.selectedQuestion.id, {
      answerText: this.answerContent,
      answerImageUrl: imageUrl
    }).subscribe({
      next: () => {
        this.loadQuestions();
        this.blurAndHide('viewQuestionModal');
      },
      error: (err: any) => alert('Cevap gönderilemedi: ' + (err.error?.message || ''))
    });
  }

  private blurAndHide(modalId: string) {
    // aria-hidden focus uyarısını önle: önce blur, sonra hide
    if (document.activeElement instanceof HTMLElement) {
      document.activeElement.blur();
    }
    const modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    if (modal) modal.hide();
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPageCount }, (_, i) => i + 1);
  }

  goToPage(p: number) {
    this.pageNumber = p;
    this.loadQuestions();
  }
}
