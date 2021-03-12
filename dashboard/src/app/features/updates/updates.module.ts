import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UpdatesComponent } from './pages/updates/updates.component';
import { UpdatesRoutingModule } from './updates-routing.module';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { AppMaterialModule } from 'src/app/app-material.module';
import { PlatformBadgeComponent } from './components/platform-badge/platform-badge.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';



@NgModule({
  declarations: [UpdatesComponent, PlatformBadgeComponent],
  imports: [
    CommonModule,
    UpdatesRoutingModule,
    ScrollingModule,
    AppMaterialModule,
    FontAwesomeModule
  ]
})
export class UpdatesModule { }
