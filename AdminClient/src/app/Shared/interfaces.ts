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

  export interface Commit{
    id : any | null;
    userName : string;
    approverName : string | null
    userId : number;
    userChallenge : UserChallenge;
    challengeId : number;
    screenshotPath : string;
    commitDateTime : Date;
    status : string;
    message : string;
}

export interface Challenge{
    id : any | null;
    typeId : string;
    typeName : string;
    name : string;
    description : string;
    reward : number;
    availableFrom : Date;
    availableTo : Date;
    creatorId : number;
    creatorName : string;
    status : string;
    imagePath : string;
    thumbnailPath : string;
}

export interface Alert{
  type: string,
  message: string
}

export interface UserChallenge{
  id : any | null;
  challengeId : number;
  challenge : Challenge;
  status : string;
  startedAt : Date;
  endedAt : Date;
  approvedAt : Date;
  approverName : string;
}

export interface ChallengeType{
  id : string;
  name : string;
}

export interface SortConfig{
  sortBy: string,
  isSortAscending: boolean
}

export interface SortEvent<T> {
  column: SortColumn<T>;
  direction: SortDirection;
}

export type SortColumn<T> = keyof T | '';
export type SortDirection = 'asc' | 'desc' | '';

export interface Achievement{
  id : any | null;
  typeId : string;
  typeName : string;
  name : string;
  streak : number;
  imagePath : string;
  thumbnailPath : string;
}