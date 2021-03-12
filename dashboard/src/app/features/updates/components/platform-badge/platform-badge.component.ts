import { Platform } from 'src/app/models/updates';
import { Component, Input, OnInit } from '@angular/core';
import { faFacebook, faTwitter, IconDefinition } from '@fortawesome/free-brands-svg-icons';
import { faRss } from '@fortawesome/free-solid-svg-icons';
import { IconProp } from '@fortawesome/fontawesome-svg-core';

@Component({
  selector: 'app-platform-badge',
  templateUrl: './platform-badge.component.html',
  styleUrls: ['./platform-badge.component.scss'],
})
export class PlatformBadgeComponent implements OnInit {
  @Input()
  platform: Platform  | undefined;

  icon: IconDefinition = faFacebook;

  constructor() {}

  ngOnInit() {
    this.icon = this.getIcon();
  }

  private getIcon(): IconDefinition {
    switch (this.platform) {
      case Platform.Facebook:
        return faFacebook;
      case Platform.Twitter:
        return faTwitter;
      case Platform.Feeds:
    }
    return faRss;
  }
}
