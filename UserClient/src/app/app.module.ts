import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from './Shared/shared.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { authInterceptorProviders } from './helpers/auth.interceptor';
import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt';
import { AuthGuard } from './helpers/auth.guard';

import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ChallengeDetailComponent } from './challenge-detail/challenge-detail.component';
import { LoginRegisterGuard } from './helpers/login-register.guard';
import { UserChallengesComponent } from './user-challenges/user-challenges.component';
import { CommitDetailComponent } from './commit-detail/commit-detail.component';
import { UserChallengeDetailComponent } from './user-challenge-detail/user-challenge-detail.component';
import { CommitsComponent } from './commits/commits.component';
import { FileUploadComponent } from './file-upload/file-upload.component';
import { NgbAlertModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ProfileComponent } from './profile/profile.component';
import { ChallengesComponent } from './challenges/challenges.component';
import { NgbdSortableHeader } from './Shared/classes';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { AchievementsComponent } from './achievements/achievements.component';
import { AgmCoreModule } from '@agm/core';
import { Config } from './config';
import { SettingsComponent } from './settings/settings.component';
import { VenuesComponent } from './venues/venues.component';
import { VenueDetailsComponent } from './venue-details/venue-details.component';

@NgModule({
  declarations: [
    AppComponent,
    RegisterComponent,
    LoginComponent,
    ChallengesComponent,
    ChallengeDetailComponent,
    UserChallengesComponent,
    CommitDetailComponent,
    UserChallengeDetailComponent,
    CommitsComponent,
    FileUploadComponent,
    ProfileComponent,
    NgbdSortableHeader,
    NavMenuComponent,
    AchievementsComponent,
    SettingsComponent,
    VenuesComponent,
    VenueDetailsComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    SharedModule,
    AppRoutingModule,
    HttpClientModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatInputModule,
    BrowserAnimationsModule,
    MatCheckboxModule,
    MatRadioModule,
    NgbAlertModule,
    NgbModule,
    AgmCoreModule.forRoot({
      apiKey: Config.keys.map,
      libraries: ["places", "geometry"]
    })
  ],
  providers: [
    authInterceptorProviders,
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    JwtHelperService,
    AuthGuard,
    LoginRegisterGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
