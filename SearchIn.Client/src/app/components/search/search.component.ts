import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { SearchHubService } from "../../services/search-hub.service";

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements AfterViewInit {

  private startUrl: string;
  private searchString: string;
  private countUrls: number;
  private countThreads: number;

  @ViewChild('SearchProgressBar') SearchProgressBar;
  private currentProgressValue: number;

  constructor(private searchHubService: SearchHubService) {
    this.currentProgressValue = 0;
  }

  ngAfterViewInit() {
    this.SearchProgressBar.nativeElement.addEventListener('mdl-componentupgraded', function () {
      this.MaterialProgress.setProgress(this.currentProgressValue);
    });
  }

  private updateProgress() {
    if (this.currentProgressValue >= 0 && this.currentProgressValue <= 100) {
      this.SearchProgressBar.nativeElement.MaterialProgress.setProgress(this.currentProgressValue);
    }
  }

  start() {
    console.log("start");
    /*this.searchHubService.connect().then(() => {
      this.searchHubService.startSearch(this.startUrl, this.searchString, this.countUrls, this.countThreads);
    });*/
  }

  pause() {
    console.log("pause");
    //this.searchHubService.pauseSearch();
  }

  stop() {
    console.log("stop");
    //this.searchHubService.stopSearch();
    //this.searchHubService.disconnect();
  }

}
