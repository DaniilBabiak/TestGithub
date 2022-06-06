import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AchievementsComponent } from './achievements/achievements.component';
import { ChallengeDetailComponent } from './challenge-detail/challenge-detail.component';
import { ChallengesComponent } from './challenges/challenges.component';
import { CommitDetailComponent } from './commit-detail/commit-detail.component';
import { CommitsComponent } from './commits/commits.component';
import { AuthGuard } from './helpers/auth.guard';
import { LoginRegisterGuard } from './helpers/login-register.guard';
import { LoginComponent } from './login/login.component';
import { ProfileComponent } from './profile/profile.component';
import { RegisterComponent } from './register/register.component';
import { UserChallengeDetailComponent } from './user-challenge-detail/user-challenge-detail.component';
import { UserChallengesComponent } from './user-challenges/user-challenges.component';
import { VenueDetailsComponent } from './venue-details/venue-details.component';
import { VenuesComponent } from './venues/venues.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [LoginRegisterGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [LoginRegisterGuard] },
  { path: '', redirectTo: '/challenges', pathMatch: 'full' },
  { path: 'challenges', component: ChallengesComponent },
  { path: 'userChallenges', component: UserChallengesComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: 'userChallenge/:userChallengeId/commit/:id/details', component: CommitDetailComponent, canActivate: [AuthGuard] },
  { path: 'userChallenge/:id/details', component: UserChallengeDetailComponent, canActivate: [AuthGuard] },
  { path: 'userChallenge/:id/commits', component: CommitsComponent, canActivate: [AuthGuard] },
  { path: 'challenge/:id/details', component: ChallengeDetailComponent, canActivate: [AuthGuard] },
  { path: 'profile/achievements', component: AchievementsComponent, canActivate: [AuthGuard] },
  { path: 'venues', component: VenuesComponent, canActivate: [AuthGuard] },
  { path: 'venue/:id', component: VenueDetailsComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
