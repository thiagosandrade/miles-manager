import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

import { TranslatePipe } from '@ngx-translate/core';

import { forkJoin, forkJoin as rxForkJoin } from 'rxjs';

import { UserRolesApiService } from './user-roles-api.service';
import { Role } from './models/user-roles.models';
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatIconModule } from "@angular/material/icon";
import { SnackbarService } from '../../core/services/snackbar.service';

@Component({
  standalone: true,
  selector: 'app-edit-user-roles-dialog',
  templateUrl: './edit-user-roles-dialog.component.html',
  styleUrls: ['./edit-user-roles-dialog.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    TranslatePipe,
    MatCheckboxModule,
    MatIconModule
]
})
export class EditUserRolesDialogComponent implements OnInit {

  private readonly fb = inject(FormBuilder);

  private readonly dialogRef =
    inject(MatDialogRef<EditUserRolesDialogComponent>);

  private readonly userRolesApi =
    inject(UserRolesApiService);

  private snackbarService = inject(SnackbarService);
    
  readonly loading = signal(false);

  readonly allRoles = signal<Role[]>([]);

  private originalRoles: string[] = [];

  readonly selectedRoles = signal<string[]>([]);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      userId: string;
    }) {
  }

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {

    this.loading.set(true);

    forkJoin({
      roles: this.userRolesApi.listRoles(),
      userRoles: this.userRolesApi.getUserRoles(this.data.userId)
    }).subscribe({

      next: result => {

        this.allRoles.set(result.roles);

        this.originalRoles =
          result.userRoles.map(x => x.roleId);

        this.originalRoles = result.userRoles.map(x => x.roleId);

        this.selectedRoles.set([...this.originalRoles]);

        this.loading.set(false);

      },

      error: () => {

        this.loading.set(false);

      }

    });

  }

  isSelected(roleId: string): boolean {

    return this.selectedRoles().includes(roleId);

  }

  toggleRole(roleId: string, checked: boolean): void {

    const roles = [...this.selectedRoles()];

    if (checked) {

      if (!roles.includes(roleId)) {
        roles.push(roleId);
      }

    } else {

      const index = roles.indexOf(roleId);

      if (index >= 0) {
        roles.splice(index, 1);
      }

    }

    this.selectedRoles.set(roles);

  }

  save(): void {

    this.loading.set(true);

    const current = this.selectedRoles();

    const added =
      current.filter(x => !this.originalRoles.includes(x));

    const removed =
      this.originalRoles.filter(x => !current.includes(x));

    const requests = [

      ...added.map(roleId =>
        this.userRolesApi.assignRole({
          userId: this.data.userId,
          roleId
        }).subscribe({
            next: () => {
              this.snackbarService.success('User role added succesfully.', 3000);
              this.loading.set(false)
            },
            error: (ex) => {
              this.snackbarService.error(`Could not add user role. ${ex.error.title}`, 3000);
              this.loading.set(false)
            } 
          })
      ),

      ...removed.map(roleId =>
        this.userRolesApi.removeRole({
          userId: this.data.userId,
          roleId
        }).subscribe({
            next: () => {
              this.snackbarService.success('User role removed succesfully.', 3000);
              this.loading.set(false)
            },
            error: (ex) => {
              this.snackbarService.error(`Could not remove user role. ${ex.error.title}`, 3000);
              this.loading.set(false)
            } 
          })
      )

    ];

    if (!requests.length) {

      this.loading.set(false);

      this.dialogRef.close(false);

      return;

    }

    forkJoin(requests).subscribe({

      next: () => {

        this.loading.set(false);

        this.dialogRef.close(true);

      },

      error: () => {

        this.loading.set(false);

      }

    });

  }

  cancel(): void {

    this.dialogRef.close(false);

  }

}