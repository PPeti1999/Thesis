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
      goalType: [0], // auto-calculated
      activityMultiplier: [1.2, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.accountClient.getProfile().subscribe({
      next: (profile: UserProfileResponseDto) => {
        if (profile) {
          // Base values
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

          // Handle activityMultiplier with valid rounding
          const rawMultiplier = profile.activityMultiplier ?? 1.2;
          const validMultipliers = [1.2, 1.375, 1.55, 1.725, 1.9];
          const roundedMultiplier = validMultipliers.find(m =>
            Math.abs(m - rawMultiplier) < 0.01
          ) ?? 1.2;

          this.profileForm.patchValue({ activityMultiplier: roundedMultiplier });

          // Calculate goal type
          this.updateGoalType();
        }

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

    let goalType = 0;
    if (goalWeight < weight) goalType = 2;
    else if (goalWeight > weight) goalType = 1;

    this.profileForm.patchValue({ goalType }, { emitEvent: false });
  }

  submit(): void {
    if (this.profileForm.valid) {
      const dto = this.profileForm.value as UpdateUserProfileDto;

      const validMultipliers = [1.2, 1.375, 1.55, 1.725, 1.9];
      const activityMultiplier = dto.activityMultiplier ?? 1.2;
      const roundedMultiplier = validMultipliers.find(m =>
        Math.abs(m - activityMultiplier) < 0.01
      ) ?? 1.2;

      dto.activityMultiplier = roundedMultiplier;

      this.accountClient.updateProfile(dto).subscribe({
        next: () => this.router.navigate(['/dailynote']),
        error: err => console.error('Profile update failed:', err)
      });
    }
  }
}