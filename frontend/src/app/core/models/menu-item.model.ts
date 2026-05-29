export interface MenuItem {
  label: string;
  icon: string;
  route?: string;
  roles: string[];
  children?: MenuItem[];
}

export interface UserProfile {
  name: string;
  role: string;
  initials: string;
  avatarColor?: string;
}
