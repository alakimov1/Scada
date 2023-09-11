import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { EventsModule } from './events/events.module';
import { TrendsModule } from './trends/trends.module';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { StateComponent } from './state/state.component';
import { SettingsComponent } from './settings/settings.component';
import { Strings } from '../common/strings';
import { TrendsComponent } from './trends/trends.component';
import { EventsComponent } from './events/events.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    StateComponent,
    SettingsComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    EventsModule,
    TrendsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'state', component: StateComponent },
      { path: 'events', component: EventsComponent },
      { path: 'trends', component: TrendsComponent },
      { path: 'settings', component: SettingsComponent },
    ]),

  ],
  providers: [Strings],
  bootstrap: [AppComponent]
})
export class AppModule { }
