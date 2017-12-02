import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from "rxjs/Observable";
import 'signalr';
import { environment } from '../../environments/environment';
import { Url } from "../models/url";
import { UrlState } from "../models/url-state";
declare var jquery: any;
declare var $: any;

@Injectable()
export class SearchHubService {

  private readonly hubUrl: string;
  private readonly hubName: string;

  private readonly SignalrConnection: any;
  private readonly ChatProxy: any;
  private ConnectionId: any;

  public readonly onConnectedEvent: EventEmitter<void>;
  public readonly onUrlStateChanged: EventEmitter<UrlState>;
  public readonly onNewUrlListFound: EventEmitter<Url[]>;
  public readonly onErrorFound: EventEmitter<string>;

  constructor() {
    this.hubUrl = environment.hubUrl;
    this.hubName = environment.hubName;

    this.SignalrConnection = $.hubConnection(this.hubUrl, {
      useDefaultPath: false
    });
    this.ChatProxy = this.SignalrConnection.createHubProxy(this.hubName);

    this.onConnectedEvent = new EventEmitter<void>();
    this.onUrlStateChanged = new EventEmitter<UrlState>();
    this.onNewUrlListFound = new EventEmitter<Url[]>();
    this.onErrorFound = new EventEmitter<string>();

    this.registerEvents();
  }

  private registerEvents(): void {
    let self = this;

    this.ChatProxy.on('onConnected', function () {
      console.log("onConnected");
      self.onConnectedEvent.emit();
    });

    this.ChatProxy.on('onUrlStateChanged', function (urlState: UrlState) {
      console.log("onUrlStateChanged");
      self.onUrlStateChanged.emit(urlState);
    });

    this.ChatProxy.on('onNewUrlListFound', function (urlList: Url[]) {
      console.log("onNewUrlListFound");
      self.onNewUrlListFound.emit(urlList);
    });

    this.ChatProxy.on('onErrorFound', function (errorMessage: string) {
      console.log("onErrorFound");
      self.onErrorFound.emit(errorMessage);
    });
  }

  async connect(): Promise<any> {
    await this.SignalrConnection.start().done((data: any) => {
      this.ConnectionId = this.SignalrConnection.id;
      console.log('Connection estabilished. Connection id: ' + this.ConnectionId);
    }).fail((error) => {
      console.log('Could not connect to hub. Error: ' + error);
    });
  }

  disconnect() {
    this.SignalrConnection.stop();
  }

  startSearch(startUrl: string, searchString: string, countUrls: number, countThreads: number) {
    this.ChatProxy.invoke('StartSearch', startUrl, searchString, countUrls, countThreads).done(function () {
      console.log('Invocation of StartSearch on server succeeded.');
    }).fail(function (error) {
      console.log('Invocation of StartSearch on server failed. Error: ' + error);
    });
  }

  pauseSearch() {
    this.ChatProxy.invoke('PauseSearch').done(function () {
      console.log('Invocation of PauseSearch on server succeeded.');
    }).fail(function (error) {
      console.log('Invocation of PauseSearch on server failed. Error: ' + error);
    });
  }

  resumeSearch() {
    this.ChatProxy.invoke('ResumeSearch').done(function () {
      console.log('Invocation of ResumeSearch on server succeeded.');
    }).fail(function (error) {
      console.log('Invocation of ResumeSearch on server failed. Error: ' + error);
    });
  }

  stopSearch() {
    this.ChatProxy.invoke('StopSearch').done(function () {
      console.log('Invocation of StopSearch on server succeeded.');
    }).fail(function (error) {
      console.log('Invocation of StopSearch on server failed. Error: ' + error);
    });
  }

}