import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AppMaterialModule } from './app-material.module';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PostsListenerComponent } from './components/posts-listener/posts-listener.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LayoutComponent } from './components/layout/layout.component';
import { ScraperComponent } from './components/scraper/scraper.component';
import { TelegramComponent } from './components/telegram/telegram.component';
import { LinkComponent } from './components/link/link.component';

@NgModule({
  declarations: [AppComponent, PostsListenerComponent, LayoutComponent, ScraperComponent, TelegramComponent, LinkComponent],
  imports: [
    AppRoutingModule,
    AppMaterialModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
