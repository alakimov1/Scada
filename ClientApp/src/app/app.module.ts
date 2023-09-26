import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { EventsModule } from './events/events.module';
import { TrendsModule } from './trends/trends.module';
import { GroupModule } from './group/group.module';

import { GroupComponent } from './group/group.component';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { Strings } from '../common/strings';
import { TrendsComponent } from './trends/trends.component';
import { EventsComponent } from './events/events.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    EventsModule,
    TrendsModule,
    GroupModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'group/:id', component: GroupComponent },
      { path: 'events', component: EventsComponent },
      { path: 'trends', component: TrendsComponent },
    ]),

  ],
  providers: [Strings],
  bootstrap: [AppComponent]
})
export class AppModule { }
