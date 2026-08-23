import { Component, inject, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';

import {
  MatDialogRef,
  MatDialogModule,
  MAT_DIALOG_DATA
} from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ProgramsApiService } from './programs-api.service';
import { ProgramCreateRequest, ProgramUpdateRequest } from './models/program.model';
import { MatSelectModule } from "@angular/material/select";
import { TranslatePipe } from '@ngx-translate/core';

interface ProgramDialogData {
  operation: 'create' | 'update';
  programId?: string;
}

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
    MatSelectModule,
    TranslatePipe
],
  templateUrl: './create-program-dialog.component.html',
  styleUrls: ['./create-program-dialog.component.scss']
})

export class CreateProgramDialogComponent implements OnInit {

    fb = inject(FormBuilder);
    api = inject(ProgramsApiService); 
    dialogRef = inject(MatDialogRef<CreateProgramDialogComponent>);
    loading = false;
    data = inject<ProgramDialogData>(MAT_DIALOG_DATA);
    
    form = this.fb.group({
        id: [''],
        programCode: ['', Validators.required],
        name: ['', Validators.required],
        programType: [0, Validators.required],
        website: [''],
        logo: [''],
        referencePricePerThousand: [
            0,
            [Validators.required, Validators.min(0)]
        ],
        isActive: [true],
        currencyCode: ['EUR', Validators.required]
    });

    ngOnInit(): void {
        console.log('operation:', this.data?.operation);
        if (this.data?.operation === 'update' && this.data?.programId) {
            this.api.getProgram(this.data.programId).subscribe({
                next: program => {
                    this.form.patchValue(program);
                }
            });
        }
        else {
            this.form.reset();
        }
    }

    save(): void {

        if (this.form.invalid) {
            return;
        }

        this.loading = true;

        if(this.data?.operation === 'update' && this.data?.programId) {
            const request: ProgramUpdateRequest = {
                id: this.data.programId!,
                programCode: this.form.value.programCode!,
                name: this.form.value.name!,
                programType: Number(this.form.value.programType),
                website: this.form.value.website ?? null,
                logo: this.form.value.logo ?? null,
                referencePricePerThousand: Number(this.form.value.referencePricePerThousand),
                isActive: this.form.value.isActive ?? true,
                currencyCode: this.form.value.currencyCode!
            };

            this.api.updateProgram(this.data.programId!, request).subscribe({

                next: () => {
                this.loading = false;
                this.dialogRef.close(true);
                },

                error: () => {
                this.loading = false;
                }

            });
        }
        else {
            const request: ProgramCreateRequest = {
                programCode: this.form.value.programCode!,
                name: this.form.value.name!,
                programType: Number(this.form.value.programType),
                website: this.form.value.website ?? null,
                logo: this.form.value.logo ?? null,
                referencePricePerThousand: Number(this.form.value.referencePricePerThousand),
                isActive: this.form.value.isActive ?? true,
                currencyCode: this.form.value.currencyCode!
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
        

    }

    close(): void {

        this.dialogRef.close();

    }

}