import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { AppMaterialModule } from './app-material.module';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { ScrollingModule as ExperimentalScrollingModule } from '@angular/cdk-experimental/scrolling';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormatPipe } from './pipes/format.pipe';

const modules = [
  AppMaterialModule,
  CommonModule,
  FontAwesomeModule,
  ScrollingModule,
  ExperimentalScrollingModule,
];

const pipes = [FormatPipe];

@NgModule({
  declarations: [...pipes],
  imports: [...modules],
  exports: [...modules, ...pipes],
})
export class SharedModule {}
