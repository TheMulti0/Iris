import { Component, OnInit } from '@angular/core';
import { ScraperService } from '../../services/scraper.service';

@Component({
  selector: 'app-scraper',
  templateUrl: './scraper.component.html',
  styleUrls: ['./scraper.component.scss']
})
export class ScraperComponent implements OnInit {

  constructor(
    private scraperService: ScraperService
  ) { }

  ngOnInit(): void {
  }

}
