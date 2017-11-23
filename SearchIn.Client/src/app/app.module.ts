import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';

import { MainMenuComponent } from './components/main-menu/main-menu.component';
import { SearchComponent } from './components/search/search.component';

import { FormsModule } from '@angular/forms';
import { SearchHubService } from './services/search-hub.service';


@NgModule({
  declarations: [
    MainMenuComponent,
    SearchComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [
    SearchHubService
  ],
  bootstrap: [MainMenuComponent]
})
export class AppModule { }
