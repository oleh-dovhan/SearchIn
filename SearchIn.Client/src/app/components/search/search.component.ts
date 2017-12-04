import { Component, OnInit, OnDestroy, AfterViewInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { environment } from "../../../environments/environment";
import { SearchHubService } from "../../services/search-hub.service";
import { Url } from "../../models/url";
import { UrlState } from "../../models/url-state";

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit, OnDestroy, AfterViewInit {

  private readonly maxCountUrls: number;
  private readonly maxCountThreads: number;

  private startUrl: string;
  private searchString: string;
  private countUrls: number;
  private countThreads: number;

  @ViewChild('SearchProgressBar') SearchProgressBar;
  private currentProgressValue: number;

  @ViewChild('ErrorDialog') ErrorDialog;
  private errorMessage: string;

  private urlList: Url[];

  private isConnectionEstablished: boolean;
  private isProcessPaused: boolean;

  constructor(private searchHubService: SearchHubService, private cdRef: ChangeDetectorRef) {
    this.maxCountUrls = environment.maxCountUrls;
    this.maxCountThreads = environment.maxCountThreads;

    this.currentProgressValue = 0;

    this.searchHubService.onUrlStateChanged.subscribe((data: UrlState) => {
      this.onUrlStateChangedHandler(data);
    });
    this.searchHubService.onNewUrlListFound.subscribe((data: Url[]) => {
      this.onNewUrlListFound(data);
    });
    this.searchHubService.onErrorFound.subscribe((data: string) => {
      this.onErrorFoundHandler(data);
    });

    this.urlList = [];

    this.isConnectionEstablished = false;
    this.isProcessPaused = false;
  }

  ngOnInit() {
    this.searchHubService.connect().then(() => {
      this.isConnectionEstablished = true;
    });
  }

  ngOnDestroy() {
    if (this.isConnectionEstablished) {
      this.searchHubService.disconnect();
    }
  }

  start() {
    if (this.checkConnection()) {
      if (this.isProcessPaused) {
        this.searchHubService.resumeSearch();
        this.isProcessPaused = false;
      }
      else {
        if (this.validateForm()) {
          this.currentProgressValue = 0;
          this.updateProgress();
          this.urlList = [];
          this.searchHubService.startSearch(this.startUrl, this.searchString, this.countUrls, this.countThreads);
        }
      }
    }
  }

  pause() {
    if (this.checkConnection()) {
      this.isProcessPaused = true;
      this.searchHubService.pauseSearch();
    }
  }

  stop() {
    if (this.checkConnection()) {
      this.isProcessPaused = false;
      this.searchHubService.stopSearch();
    }
  }

  private checkConnection(): boolean {
    if (!this.isConnectionEstablished) {
      this.showErrorDialog("Connection to server not estabilished.");
    }
    return this.isConnectionEstablished;
  }

  private validateForm(): boolean {
    let isAllFieldsFilled = this.startUrl != undefined
      && this.searchString != undefined
      && this.countUrls != undefined
      && this.countThreads != undefined
      && this.startUrl != ""
      && this.searchString != ""
    let isCountUrlsValid = this.countUrls <= this.maxCountUrls
      && this.countUrls > 0;
    let isCountThreadsValid = this.countThreads <= this.maxCountThreads
      && this.countThreads > 0;
    if (!isAllFieldsFilled) {
      this.showErrorDialog("Form is not valid. All fields must be filled.");
    }
    else if (!isCountUrlsValid) {
      this.showErrorDialog("Form is not valid. Count URLs must be more than 0 and less than " + this.maxCountUrls + ".");
    }
    else if (!isCountThreadsValid) {
      this.showErrorDialog("Form is not valid. Count threads must be more than 0 and less than " + this.maxCountThreads + ".");
    }
    return isAllFieldsFilled && isCountUrlsValid && isCountThreadsValid;
  }

  private onUrlStateChangedHandler(data: UrlState) {
    let url = this.urlList.find(url => url.Id == data.Id);
    if (url != undefined) {
      url.ScanState = data.ScanState;
      this.cdRef.detectChanges();
    }
  }
  private onNewUrlListFound(data: Url[]) {
    this.currentProgressValue += 100 * data.length / this.countUrls;
    this.updateProgress();
    this.urlList = this.urlList.concat(data);
    this.cdRef.detectChanges();
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
    this.cdRef.detectChanges();
  }
  private closeErrorDialog() {
    this.errorMessage = "";
    this.ErrorDialog.nativeElement.close();
  }

}
