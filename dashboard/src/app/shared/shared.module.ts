import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppMaterialModule } from './app-material.module';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { ScrollingModule as ExperimentalScrollingModule } from '@angular/cdk-experimental/scrolling';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

const modules = [
  AppMaterialModule,
  CommonModule,
  FontAwesomeModule,
  ScrollingModule,
  ExperimentalScrollingModule,
];

@NgModule({
  declarations: [],
  imports: [...modules],
  exports: [...modules],
})
export class SharedModule {}
