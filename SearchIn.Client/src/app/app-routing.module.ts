import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SearchComponent } from './components/search/search.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '/search'
  }, {
    path: 'search',
    component: SearchComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
