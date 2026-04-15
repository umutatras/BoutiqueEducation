import { Component, OnInit, OnDestroy, ElementRef, ViewChild, ChangeDetectorRef, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ChatService, ChatMessage } from '../../services/chat';
import { AuthService } from '../../services/auth';
import { UserService, Contact } from '../../services/user';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
  encapsulation: ViewEncapsulation.None
})
export class Chat implements OnInit, OnDestroy {
  @ViewChild('messageBox') private messageBox!: ElementRef<HTMLDivElement>;

  messages: ChatMessage[] = [];
  messageContent: string = '';
  isConnected: boolean = false;
  isConnecting: boolean = false;
  currentUserId: string = '';
  isSending: boolean = false;

  contacts: Contact[] = [];
  selectedContact: Contact | null = null;
  isLoadingContacts: boolean = false;
  isLoadingHistory: boolean = false;

  private subs: Subscription[] = [];

  constructor(
    public chatService: ChatService,
    public authService: AuthService,
    private userService: UserService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const decoded = this.authService.getDecodedToken();
    if (decoded) {
      const nameIdKey = Object.keys(decoded).find(k =>
        k.endsWith('/nameidentifier') || k === 'sub' || k === 'nameid'
      );
      this.currentUserId = nameIdKey ? decoded[nameIdKey] : '';
    }

    this.isConnecting = true;
    this.chatService.startConnection()
      .then(() => this.isConnecting = false)
      .catch(() => this.isConnecting = false);

    this.loadContacts();

    this.subs.push(
      this.chatService.messages$.subscribe(msgs => {
        this.messages = msgs;
        this.cdr.detectChanges();      // Angular'a değişikliği bildir
        this.scrollToBottom();         // Mesaj gelince hemen kaydır
      }),
      this.chatService.connected$.subscribe(c => this.isConnected = c)
    );
  }

  loadContacts() {
    this.isLoadingContacts = true;
    this.userService.getContacts().subscribe({
      next: (res: any) => {
        this.contacts = res?.data || res || [];
        this.isLoadingContacts = false;
      },
      error: () => this.isLoadingContacts = false
    });
  }

  selectContact(contact: Contact) {
    if (this.selectedContact?.id === contact.id) return;
    this.selectedContact = contact;
    this.isLoadingHistory = true;
    this.chatService.loadHistory(contact.id);
    setTimeout(() => {
      this.isLoadingHistory = false;
      this.scrollToBottom();
    }, 500);
  }

  scrollToBottom(): void {
    // requestAnimationFrame ile DOM'un render'ı bitince çalış
    requestAnimationFrame(() => {
      try {
        const el = this.messageBox?.nativeElement;
        if (el) el.scrollTop = el.scrollHeight;
      } catch (_) {}
    });
  }

  sendMessage() {
    const content = this.messageContent.trim();
    if (!content || !this.selectedContact || !this.isConnected || this.isSending) return;

    this.isSending = true;
    const backup = this.messageContent;
    this.messageContent = '';

    this.chatService.sendMessage(this.selectedContact.id, content)
      .then(() => this.isSending = false)
      .catch(err => {
        this.isSending = false;
        this.messageContent = backup;
        console.error('Mesaj gönderilemedi:', err);
      });
  }

  isOwnMessage(msg: ChatMessage): boolean {
    return msg.senderId === this.currentUserId;
  }

  ngOnDestroy() {
    this.subs.forEach(s => s.unsubscribe());
    this.chatService.stopConnection();
    this.chatService.clearMessages();
  }
}
