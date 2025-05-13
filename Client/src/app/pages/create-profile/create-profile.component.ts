import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountClient, UpdateUserProfileDto, UserProfileResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-profile',
  standalone: false,
  templateUrl: './create-profile.component.html',
  styleUrl: './create-profile.component.css'
})
export class CreateProfileComponent implements OnInit {
  profileForm: FormGroup;
  loading = true;

  constructor(
    private fb: FormBuilder,
    private accountClient: AccountClient,
    private router: Router
  ) {
    this.profileForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      age: [0, [Validators.required, Validators.min(1)]],
      height: [0, [Validators.required, Validators.min(1)]],
      weight: [0, [Validators.required, Validators.min(1)]],
      goalWeight: [0, [Validators.required, Validators.min(1)]],
      bodyFat: [0, [Validators.required, Validators.min(1)]],
      isFemale: [false],
      goalType: [0], // calculated only
      activityMultiplier: [1.2, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.accountClient.getProfile().subscribe({
      next: (profile: UserProfileResponseDto) => {
        if (profile) {
          // Patch basic values
          this.profileForm.patchValue({
            firstName: profile.firstName ?? '',
            lastName: profile.lastName ?? '',
            age: profile.age ?? 0,
            height: profile.height ?? 0,
            weight: profile.weight ?? 0,
            goalWeight: profile.goalWeight ?? 0,
            bodyFat: profile.bodyFat ?? 0,
            isFemale: profile.isFemale ?? false
          });
  
          // Safe defaults for calculation
          const weight = profile.weight ?? 0;
          const height = profile.height ?? 0;
          const age = profile.age ?? 0;
          const isFemale = profile.isFemale ?? false;
          const targetCalorie = profile.targetCalorie ?? 0;
  
          let multiplier = 1.2; // default
  
          if (targetCalorie > 0 && weight > 0 && height > 0 && age > 0) {
            const bmr =
              10 * weight +
              6.25 * height -
              5 * age +
              (isFemale ? -161 : 5);
  
            if (bmr > 0) {
              multiplier = Math.round((targetCalorie / bmr) * 100) / 100;
            }
          }
  
          this.profileForm.patchValue({ activityMultiplier: multiplier });
  
          this.updateGoalType(); // calculate goalType based on weight and goalWeight
        }
  
        // Subscribe to live changes for auto goal type calculation
        this.profileForm.get('weight')!.valueChanges.subscribe(() => this.updateGoalType());
        this.profileForm.get('goalWeight')!.valueChanges.subscribe(() => this.updateGoalType());
  
        this.loading = false;
      },
      error: err => {
        console.error('Profile loading failed:', err);
        this.loading = false;
      }
    });
  }
  

  updateGoalType(): void {
    const weight = this.profileForm.get('weight')?.value ?? 0;
    const goalWeight = this.profileForm.get('goalWeight')?.value ?? 0;

    let goalType = 0; // maintain
    if (goalWeight < weight) goalType = 2; // cutting
    else if (goalWeight > weight) goalType = 1; // bulking

    this.profileForm.patchValue({ goalType }, { emitEvent: false });
  }

  submit(): void {
    if (this.profileForm.valid) {
      const dto = this.profileForm.value as UpdateUserProfileDto;
      this.accountClient.updateProfile(dto).subscribe({
        next: () => this.router.navigate(['/dailynote']),
        error: err => console.error('Profile update failed:', err)
      });
    }
  }
}