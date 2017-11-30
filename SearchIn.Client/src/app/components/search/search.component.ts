import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { SearchHubService } from "../../services/search-hub.service";
import { Url } from "../../models/url";
import { UrlState } from "../../models/url-state";

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

  constructor(private searchHubService: SearchHubService) {
    this.currentProgressValue = 0;
    this.urlList = [];

    this.searchHubService.onUrlStateChanged.subscribe((data: UrlState) => {
      this.onUrlStateChangedHandler(data);
    });
    this.searchHubService.onNewUrlListFound.subscribe((data: Url[]) => {
      this.onNewUrlListFound(data);
    });
    this.searchHubService.onErrorFound.subscribe((data: string) => {
      this.onErrorFoundHandler(data);
    });
  }

  start() {
    this.searchHubService.connect().then(() => {
      this.searchHubService.startSearch(this.startUrl, this.searchString, this.countUrls, this.countThreads);
    });
  }

  pause() {
    this.searchHubService.pauseSearch();
  }

  stop() {
    this.searchHubService.stopSearch();
    this.searchHubService.disconnect();
    this.currentProgressValue = 0;
    this.updateProgress();
  }

  private onUrlStateChangedHandler(data: UrlState) {
    let url = this.urlList.find(url => url.Id == data.Id);
    url.ScanState = data.ScanState;
  }

  private onNewUrlListFound(data: Url[]) {
    this.currentProgressValue += 100 * data.length / this.countUrls;
    this.updateProgress();
    this.urlList = this.urlList.concat(data);
  }

  private onErrorFoundHandler(data: string) {
    this.showErrorDialog(data);
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

  private showErrorDialog(errorMessage: string) {
    this.errorMessage = errorMessage;
    this.ErrorDialog.nativeElement.showModal();
  }

  private closeErrorDialog() {
    this.errorMessage = "";
    this.ErrorDialog.nativeElement.close();
  }

}
