import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { SharedModule } from './Shared/shared.module';
import { AppComponent } from './app.component';
import { ChallengeDetailComponent } from './challenge-detail/challenge-detail.component';
import { ChallengesComponent } from './challenges/challenges.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { authInterceptorProviders } from './helpers/auth.interceptor';
import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt';
import { FileUploadComponent } from './file-upload/file-upload.component';
import { AuthGuard } from './helpers/auth.guard';

import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommitsComponent } from './commits/commits.component';
import { CommitDetailComponent } from './commit-detail/commit-detail.component';
import { LoginRegisterGuard } from './helpers/login-register.guard';
import { MatTableModule } from '@angular/material/table';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgbdSortableHeader } from './Shared/classes';
import { AchievementsComponent } from './achievements/achievements.component';
import { AchievementDetailsComponent } from './achievement-details/achievement-details.component';
@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatNativeDateModule,
    MatInputModule,
    BrowserAnimationsModule,
    MatCheckboxModule,
    MatRadioModule,
    MatTableModule,
    SharedModule,
    NgbModule
  ],
  declarations: [
    AppComponent,
    ChallengesComponent,
    ChallengeDetailComponent,
    RegisterComponent,
    LoginComponent,
    FileUploadComponent,
    CommitsComponent,
    CommitDetailComponent,
    NgbdSortableHeader,
    AchievementsComponent,
    AchievementDetailsComponent

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