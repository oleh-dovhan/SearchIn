import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from "rxjs/Observable";
import 'signalr';
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

  constructor() {
    this.SignalrConnection = $.hubConnection(this.hubUrl, {
      useDefaultPath: false
    });
    this.ChatProxy = this.SignalrConnection.createHubProxy(this.hubName);

    this.onConnectedEvent = new EventEmitter<void>();

    this.registerEvents();
  }

  private registerEvents(): void {
    let self = this;

    this.ChatProxy.on('onConnected', function () {
      console.log("onConnected");
      self.onConnectedEvent.emit();
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

}