import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AppMaterialModule } from './app-material.module';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { ScrollingModule as ExperimentalScrollingModule } from '@angular/cdk-experimental/scrolling';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormatPipe } from './pipes/format.pipe';
import { LayoutComponent } from './components/layout/layout.component';

const modules = [
  AppMaterialModule,
  RouterModule,
  CommonModule,
  FontAwesomeModule,
  ScrollingModule,
  ExperimentalScrollingModule,

];

const pipes = [FormatPipe];

@NgModule({
  declarations: [...pipes, LayoutComponent],
  imports: [...modules],
  exports: [...modules, ...pipes],
})
export class SharedModule {}
