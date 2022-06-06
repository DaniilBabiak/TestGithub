export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T>{
  resut: T;
  pagination: Pagination;
}

export interface Commit {
  id: any | null;
  userName: string;
  approverName: string | null
  userId: number;
  userChallenge: UserChallenge;
  userChallengeId: number;
  screenshotPath: string;
  commitDateTime: Date;
  status: string;
  message: string;
}

export interface Challenge {
  id: any | null;
  name: string;
  description: string;
  reward: number;
  availableFrom: Date;
  availableTo: Date;
  creatorId: number;
  creatorName: string;
  status: string;
  imagePath: string;
  thumbnailPicture: string;
}

export interface UserChallenge {
  id: any | null;
  challengeId: number;
  challenge: Challenge;
  status: string;
  startedAt: Date;
  endedAt: Date;
  approvedAt: Date;
  approverName: string;
}

export interface Alert {
  type: string,
  message: string
}

export interface User {
  userName: string,
  coins: number,
  firstName: string,
  lastName: string,
  about: string,
  photoPath: string,
  location: Location | null
}

export interface SortConfig {
  sortBy: string,
  isSortAscending: boolean
}

export interface SortEvent<T> {
  column: SortColumn<T>;
  direction: SortDirection;
}

export interface Notification {
  id: any;
  challengeName: string;
  body: string;
  isChecked: boolean;
}

export interface Achievement {
  id: any | null;
  typeId: string;
  typeName: string;
  name: string;
  streak: number;
  imagePath: string;
  thumbnailPath: string;
}

export interface Location {
  latitude: number,
  longitude: number
}

export interface NotificationSettings {
  sendAchievementNotification: boolean,
  sendChallengeNotification: boolean,
  sendCommitNotification: boolean,
  sendUserChallengeNotification: boolean,
  sendVenueNotification: boolean
}

export interface Venue {
  id: any | null,
  title: string,
  description: string,
  creator: User,
  date: Date,
  location: Location,
  attendees: Array<User>
}

export type SortColumn<T> = keyof T | '';
export type SortDirection = 'asc' | 'desc' | '';