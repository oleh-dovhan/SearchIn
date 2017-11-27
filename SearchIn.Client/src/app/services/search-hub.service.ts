import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from "rxjs/Observable";
import 'signalr';
import { Url } from "../models/url";
declare var jquery: any;
declare var $: any;

@Injectable()
export class SearchHubService {

  private hubUrl = 'http://localhost:49690/search';
  private hubName = 'searchhub';

  private SignalrConnection: any;
  private ChatProxy: any;
  private ConnectionId: any;

  public onConnectedEvent: EventEmitter<void>;
  public onNewUrlListFound: EventEmitter<Url[]>;

  constructor() {
    this.SignalrConnection = $.hubConnection(this.hubUrl, {
      useDefaultPath: false
    });
    this.ChatProxy = this.SignalrConnection.createHubProxy(this.hubName);

    this.onConnectedEvent = new EventEmitter<void>();
    this.onNewUrlListFound = new EventEmitter<Url[]>();

    this.registerEvents();
  }

  private registerEvents(): void {
    let self = this;

    this.ChatProxy.on('onConnected', function () {
      console.log("onConnected");
      self.onConnectedEvent.emit();
    });

    this.ChatProxy.on('onNewUrlListFound', function (urlList: Url[]) {
      console.log("onNewUrlListFound");
      urlList.forEach(el => console.log(el));
      self.onNewUrlListFound.emit(urlList);
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

  stopSearch() {
    this.ChatProxy.invoke('StopSearch').done(function () {
      console.log('Invocation of StopSearch on server succeeded.');
    }).fail(function (error) {
      console.log('Invocation of StopSearch on server failed. Error: ' + error);
    });
  }

}