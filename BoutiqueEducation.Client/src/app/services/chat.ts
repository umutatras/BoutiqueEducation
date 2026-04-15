import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from './auth';
import { environment } from '../../environments/environment';

export interface ChatMessage {
  id?: string;
  senderId: string;
  receiverId: string;
  senderName?: string;
  content: string;
  sentAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService implements OnDestroy {
  private hubUrl = environment.apiUrl.replace('/api', '') + '/chathub';
  private apiUrl = environment.apiUrl + '/messages';
  private connection!: signalR.HubConnection;

  public messages$ = new BehaviorSubject<ChatMessage[]>([]);
  public connected$ = new BehaviorSubject<boolean>(false);

  private activeContactId: string = '';

  constructor(private authService: AuthService, private http: HttpClient) {}

  startConnection(): Promise<void> {
    const token = this.authService.getToken();

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => token ?? ''
      })
      .withAutomaticReconnect()
      .build();

    // Alıcı tarafına gelecek mesaj
    this.connection.on('ReceiveMessage', (message: ChatMessage) => {
      // Aktif konuşmayla ilgiliyse ekle
      if (
        message.senderId === this.activeContactId ||
        message.receiverId === this.activeContactId
      ) {
        this.appendMessage(message);
      }
    });

    // Gönderenin kendi onayı — duplicate kontrolü ile
    this.connection.on('MessageSent', (message: ChatMessage) => {
      this.appendMessage(message);
    });

    this.connection.onreconnected(() => this.connected$.next(true));
    this.connection.onclose(() => this.connected$.next(false));

    return this.connection
      .start()
      .then(() => this.connected$.next(true))
      .catch((err) => {
        this.connected$.next(false);
        console.error('SignalR bağlantı hatası:', err);
      });
  }

  /** Kişi seçildiğinde DB'den geçmiş yükle */
  loadHistory(contactId: string): void {
    this.activeContactId = contactId;
    this.messages$.next([]);

    this.http.get<any>(`${this.apiUrl}/history/${contactId}`).subscribe({
      next: (res) => {
        const history: ChatMessage[] = res?.data || res || [];
        this.messages$.next(history);
      },
      error: (err) => console.error('Mesaj geçmişi yüklenemedi:', err)
    });
  }

  sendMessage(receiverId: string, content: string): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      // Optimistic UI YOK — MessageSent event'i gelince eklenecek
      return this.connection.invoke('SendMessage', receiverId, content);
    }
    return Promise.reject('Bağlantı yok.');
  }

  clearMessages(): void {
    this.messages$.next([]);
    this.activeContactId = '';
  }

  stopConnection(): void {
    this.connection?.stop();
    this.connected$.next(false);
  }

  ngOnDestroy(): void {
    this.stopConnection();
  }

  /** ID bazlı duplicate kontrolü ile mesaj ekle */
  private appendMessage(message: ChatMessage): void {
    const current = this.messages$.getValue();
    if (message.id && current.some(m => m.id === message.id)) return; // duplicate
    this.messages$.next([...current, message]);
  }
}
