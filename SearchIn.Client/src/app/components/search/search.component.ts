import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { SearchHubService } from "../../services/search-hub.service";
import { Url } from "../../models/url";

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

  @ViewChild('ErrorDialog') ErrorDialog;
  private errorMessage: string;

  private urlList: Url[];
  private a: number[];

  constructor(private searchHubService: SearchHubService) {
    this.currentProgressValue = 0;
    this.urlList = [];
    this.a = [1, 2, 4, 5, 6, 7, 2, 1, 9, 1, 2, 4, 5, 6, 7, 2, 1, 9, 1, 2, 4, 5, 6, 7, 2, 1, 9, 1, 2, 4, 5, 6, 7, 2, 1, 9];
    //this.searchHubService.onNewUrlListFound.subscribe((data: Url[]) => {
    //this.currentProgressValue += 100 * data.length / this.countUrls;
    //this.updateProgress();
    //this.urlList = this.urlList.concat(data);
    //});
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

  private showErrorDialog() {
    this.ErrorDialog.nativeElement.showModal();
  }

  private closeErrorDialog() {
    this.ErrorDialog.nativeElement.close();
  }

  start() {
    console.log("start");
    this.errorMessage = "nfv dnv dj fnvj dnfb ndjn fb unedi bndkjfn bin vk jdn od j rk jir i rti rjt rjt jrt kjgfn jfg jhgf kdn hf gfgn kfgj kjg nkjfn kjfn jkfnjk fngkj nkjf nkj jgnkfnhfjjkl nhl fngk h";
    this.showErrorDialog();
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
