import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChallengesComponent } from './challenges/challenges.component';
import { ChallengeDetailComponent } from './challenge-detail/challenge-detail.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './helpers/auth.guard';
import { CommitDetailComponent } from './commit-detail/commit-detail.component';
import { CommitsComponent } from './commits/commits.component';
import { LoginRegisterGuard } from './helpers/login-register.guard';
import { AchievementsComponent } from './achievements/achievements.component';
import { AchievementDetailsComponent } from './achievement-details/achievement-details.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent, canActivate: [LoginRegisterGuard] },
  { path: 'challenges', component: ChallengesComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/challenges', pathMatch: 'full' },
  { path: 'challenge/:id/details', component: ChallengeDetailComponent, canActivate: [AuthGuard] },
  { path: 'commits', component: CommitsComponent, canActivate: [AuthGuard] },
  { path: 'commit/:id/details', component: CommitDetailComponent, canActivate: [AuthGuard] },
  { path: 'achievements', component: AchievementsComponent, canActivate: [AuthGuard] },
  { path: 'achievement/:id/details', component: AchievementDetailsComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }