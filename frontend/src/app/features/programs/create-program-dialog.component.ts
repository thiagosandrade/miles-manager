import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';

import {
  MatDialogRef,
  MatDialogModule
} from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ProgramsApiService } from './programs-api.service';
import { ProgramCreateRequest } from './models/program.model';
import { MatSelectModule } from "@angular/material/select";


@Component({
  selector: 'app-create-program-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatSelectModule
],
  templateUrl: './create-program-dialog.component.html',
  styleUrls: ['./create-program-dialog.component.scss']
})

export class CreateProgramDialogComponent {

    fb = inject(FormBuilder);
    api = inject(ProgramsApiService); 
    dialogRef = inject(MatDialogRef<CreateProgramDialogComponent>);
    loading = false;

    form = this.fb.group({

        code: ['', Validators.required],

        name: ['', Validators.required],

        programType: [0, Validators.required],

        website: [''],

        logo: [''],

        referencePricePerThousand: [
            0,
            [Validators.required, Validators.min(0)]
        ],

        isActive: [true]

    });


    save(): void {

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        const request: ProgramCreateRequest = {
            code: this.form.value.code!,
            name: this.form.value.name!,
            programType: Number(this.form.value.programType),
            website: this.form.value.website ?? null,
            logo: this.form.value.logo ?? null,
            referencePricePerThousand: Number(this.form.value.referencePricePerThousand),
            isActive: this.form.value.isActive ?? true
        };

        this.api.createProgram(request).subscribe({

            next: () => {
            this.loading = false;
            this.dialogRef.close(true);
            },

            error: () => {
            this.loading = false;
            }

        });

    }

    close(): void {

        this.dialogRef.close();

    }

}