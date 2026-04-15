import { Component, AfterViewInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from '../header/header';
import { Sidebar } from '../sidebar/sidebar';
import { Footer } from '../footer/footer';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, Header, Sidebar, Footer],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout implements AfterViewInit {
  ngAfterViewInit() {
    // PerfectScrollbar hedef elementleri kontrol ederek yükle
    setTimeout(() => {
      this.loadScripts([
        'assets/plugins/sidemenu/sidemenu.js',
        'assets/js/sticky.js',
        'assets/plugins/sidebar/sidebar.js',
        'assets/plugins/p-scroll/perfect-scrollbar.js',
        'assets/plugins/p-scroll/pscroll.js',
        'assets/plugins/p-scroll/pscroll-1.js',
        'assets/js/themeColors.js',
        'assets/js/custom.js'
      ]);
    }, 100);
  }

  loadScripts(urls: string[]) {
    urls.forEach(url => {
      if (!document.querySelector(`script[src="${url}"]`)) {
        const node = document.createElement('script');
        node.src = url;
        node.type = 'text/javascript';
        node.async = false;
        // Script hata verse de diğerlerini engelleme
        node.onerror = () => console.warn(`Script yüklenemedi: ${url}`);
        document.body.appendChild(node);
      }
    });
  }
}
