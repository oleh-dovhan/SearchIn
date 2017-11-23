import { Component, OnInit } from '@angular/core';
import { SearchHubService } from "../../services/search-hub.service";

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {

  private startUrl: string;
  private searchText: string;
  private countUrls: number;
  private countThreads: number;

  constructor(private searchHubService: SearchHubService) { }

  ngOnInit() {
  }

  start() {
    console.log("start");
    this.searchHubService.connect();
  }

  pause() {
    console.log("pause");
  }

  stop() {
    console.log("stop");
    this.searchHubService.disconnect();    
  }
  
}
