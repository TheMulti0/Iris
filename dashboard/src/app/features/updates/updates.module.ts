import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UpdatesComponent } from './pages/updates/updates.component';
import { UpdatesRoutingModule } from './updates-routing.module';



@NgModule({
  declarations: [UpdatesComponent],
  imports: [
    CommonModule,
    UpdatesRoutingModule
  ]
})
export class UpdatesModule { }
