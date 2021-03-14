import { NgModule } from '@angular/core';
import { UpdatesComponent } from './pages/updates/updates.component';
import { UpdatesRoutingModule } from './updates-routing.module';
import { PlatformBadgeComponent } from './components/platform-badge/platform-badge.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { UpdateComponent } from './components/update/update.component';

@NgModule({
  declarations: [UpdatesComponent, PlatformBadgeComponent, UpdateComponent],
  imports: [SharedModule, UpdatesRoutingModule],
})
export class UpdatesModule {}
