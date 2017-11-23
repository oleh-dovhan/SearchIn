import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';

import { MainMenuComponent } from './components/main-menu/main-menu.component';
import { SearchComponent } from './components/search/search.component';


@NgModule({
  declarations: [
    MainMenuComponent,
    SearchComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [MainMenuComponent]
})
export class AppModule { }
